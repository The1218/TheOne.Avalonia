#!/usr/bin/env python3
"""Check the actual library theme resources, including hover and pressed colours."""
from pathlib import Path
import json
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
KEY = "{http://schemas.microsoft.com/winfx/2006/xaml}Key"
AV = "{https://github.com/avaloniaui}"
source = ET.parse(ROOT / "TheOne.Avalonia/Themes/Colors.axaml").getroot()

def luminance(value):
    rgb = [int(value[i:i + 2], 16) / 255 for i in (1, 3, 5)]
    linear = [c / 12.92 if c <= .04045 else ((c + .055) / 1.055) ** 2.4 for c in rgb]
    return sum(c * weight for c, weight in zip(linear, [.2126, .7152, .0722]))

report = []
for dictionary in source.find(AV + "ResourceDictionary.ThemeDictionaries"):
    colors = {node.attrib[KEY]: node.text.strip() for node in dictionary if node.tag == AV + "Color"}
    brushes = {}
    for node in dictionary:
        if node.tag != AV + "SolidColorBrush":
            continue
        value = node.attrib["Color"]
        brushes[node.attrib[KEY]] = colors[value[16:-1]] if value.startswith("{StaticResource ") else value
    pairs = [("Text", "Canvas", 4.5), ("Text", "Surface", 4.5),
             ("Text", "Subtle", 4.5), ("Text", "AccentSoft", 4.5), ("Muted", "Surface", 4.5),
             ("Muted", "Subtle", 4.5), ("Accent", "AccentSoft", 4.5), ("AccentHover", "AccentSoft", 4.5), ("AccentPressed", "AccentSoft", 3), ("Success", "SuccessSoft", 4.5),
             ("Danger", "Surface", 4.5), ("InputLine", "Surface", 3), ("Focus", "Surface", 3)]
    pairs += [("Muted", "CardTabStrip", 4.5), ("Text", "CardTabHover", 4.5)]
    pairs += [("Warning", "Subtle", 3), ("Danger", "DangerSoft", 3)]
    pairs += [("Text", "InfoCard.Footer", 4.5), ("Muted", "InfoCard.Footer", 4.5)]
    # Pie/doughnut percentages use surface ink on the four chart series fills.
    pairs += [("Surface", background, 4.5) for background in ["Accent", "Muted", "Warning", "Danger"]]
    pairs += [("OnAccent", background, 4.5) for background in
              ["Accent", "AccentHover", "AccentPressed", "Danger", "DangerHover", "DangerPressed"]]
    for foreground, background, minimum in pairs:
        values = sorted([luminance(brushes["One." + foreground]), luminance(brushes["One." + background])])
        ratio = (values[1] + .05) / (values[0] + .05)
        entry = {"theme": dictionary.attrib[KEY], "pair": f"{foreground}/{background}",
                 "minimum": minimum, "ratio": round(ratio, 2)}
        if ratio < minimum:
            raise RuntimeError(f"Contrast failed: {entry}")
        report.append(entry)

output = ROOT / "docs/theme/contrast-report.json"
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(report, indent=2))
print(f"Passed {len(report)} actual theme contrast pairs: {output}")
