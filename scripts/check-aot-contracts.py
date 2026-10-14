#!/usr/bin/env python3
"""Reject reflection bindings and keep the native consumer's theme coverage complete."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
library = root / "TheOne.Avalonia"
smoke = root / "tests/NativeAotSmoke"
preview = root / "design/PreviewHost"
x = "{http://schemas.microsoft.com/winfx/2006/xaml}"
av = "{https://github.com/avaloniaui}"
errors = []
keys = []
for folder in (library, smoke, preview):
    for path in folder.rglob("*.axaml"):
        if "obj" in path.parts or "bin" in path.parts:
            continue
        tree = ET.parse(path)
        if folder == preview and "Views" in path.relative_to(preview).parts:
            if re.search(r"\{(?:Binding|(?:[\w]+:)?CompiledBinding(?:Extension)?)\b", path.read_text()) and not tree.getroot().get(x + "DataType"):
                errors.append(f"{path.relative_to(root)}: view bindings require a typed root x:DataType")
        for element in tree.iter():
            if element.tag.endswith("}CompiledBindingExtension") and any(name in element.attrib for name in ("RelativeSource", "ElementName")):
                errors.append(f"{path.relative_to(root)}: CompiledBindingExtension source must use explicit $self / #name paths for Rider")
            if folder == preview and "Views" in path.relative_to(preview).parts:
                context = element.get("DataContext", "")
                if re.fullmatch(r"\{(?:Binding|(?:[\w]+:)?CompiledBinding(?:Extension)?)\s+#[\w]+\.DataContext\s*\}", context):
                    errors.append(f"{path.relative_to(root)}: object DataContext bridge; use a typed Model or explicit VM cast")
            if element.get(x + "CompileBindings", "").lower() == "false":
                errors.append(f"{path.relative_to(root)}: compiled bindings disabled")
            if element.tag == av + "Binding":
                errors.append(f"{path.relative_to(root)}: reflection-based Binding element")
            for value in element.attrib.values():
                if "{ReflectionBinding" in value:
                    errors.append(f"{path.relative_to(root)}: ReflectionBinding")
            if folder == library and element.tag == av + "ControlTheme" and (key := element.get(x + "Key")):
                if key.startswith("{x:Type "):
                    keys.append(("type", key[8:-1].split(":")[-1]))
                else:
                    keys.append(("string", key))
    project = "TheOne.Avalonia.csproj" if folder == library else "NativeAotSmoke.csproj" if folder == smoke else "PreviewHost.csproj"
    props = ET.parse(folder / project)
    for prop in ("AvaloniaUseCompiledBindingsByDefault", "IsAotCompatible"):
        if props.findtext(f".//{prop}", "").lower() != "true":
            errors.append(f"{folder.name}: {prop} must be true")
    for path in folder.rglob("*.cs"):
        if "obj" in path.parts or "bin" in path.parts:
            continue
        if re.search(r"\bnew\s+(?:ReflectionBinding|Binding)\s*\(|Activator\.CreateInstance|Type\.GetType\(", path.read_text()):
            errors.append(f"{path.relative_to(root)}: runtime reflection construction requires review")

manifest = (smoke / "ThemeManifest.cs").read_text()
actual = []
for key in re.findall(r'^\s*\((typeof\(global::[\w.]+\)|"[^"]+"),\s*typeof', manifest, re.M):
    actual.append(("type", key[7:-1].split(".")[-1]) if key.startswith("typeof") else ("string", key[1:-1]))
if sorted(actual) != sorted(keys):
    errors.append(f"ThemeManifest keys differ: missing {set(keys) - set(actual)}, extra {set(actual) - set(keys)}")
assets = {p.name for p in (library / "Themes/Controls").glob("*.axaml")}
if assets != set(re.findall(r'"([\w]+\.axaml)"', manifest)):
    errors.append("ThemeManifest dictionary list differs from library resources")
include_sources = {Path(element.get("Source", "")).name for element in ET.parse(library / "Themes/OneTheme.axaml").iter(av + "ResourceInclude")}
if not assets <= include_sources:
    errors.append(f"OneTheme does not include {assets - include_sources}")
if errors:
    raise SystemExit("\n".join(errors))
print(f"PASS: compiled bindings / AOT contracts, {len(assets)} dictionaries and {len(keys)} theme keys covered")
