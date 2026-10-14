#!/usr/bin/env python3
"""Check maintained Markdown links, fragments, and context budgets without UI builds."""

from pathlib import Path
import re
import sys
from urllib.parse import unquote, urlsplit


ROOT = Path(__file__).resolve().parent.parent


def markdown_files():
    yield from ROOT.glob("*.md")
    for directory in ("docs", ".agents/skills"):
        yield from (ROOT / directory).rglob("*.md")


def prose(text):
    # Code examples can contain strings that resemble Markdown links/headings.
    return re.sub(r"^```[^\n]*\n.*?^```[^\n]*$", "", text, flags=re.M | re.S)


def anchors(text):
    seen = {}
    result = set()
    for heading in re.findall(r"^#{1,6}\s+(.+?)\s*#*\s*$", prose(text), re.M):
        heading = re.sub(r"\[([^]]+)\]\([^)]*\)", r"\1", heading)
        slug = re.sub(r"[^\w\- ]", "", heading.lower()).replace(" ", "-")
        occurrence = seen.get(slug, 0)
        result.add(f"{slug}-{occurrence}" if occurrence else slug)
        seen[slug] = occurrence + 1
    return result


def main():
    failures = []
    files = sorted(set(markdown_files()))
    active_bytes = 0
    for path in files:
        relative = path.relative_to(ROOT)
        text = path.read_text(encoding="utf-8")
        size = len(text.encode("utf-8"))
        archived = "archive" in relative.parts
        if not archived:
            active_bytes += size
            limit = 2200 if path.name == "AGENTS.md" else 3000 if path.name == "SKILL.md" else 6000 if path.name == "README.md" else 12000
            if size > limit:
                failures.append(f"{relative}: {size} bytes exceeds {limit}; split by task")
        for target in re.findall(r"!?\[[^]\n]*\]\(([^)\n]+)\)", prose(text)):
            target = target.strip().split(' "', 1)[0].strip("<>")
            parsed = urlsplit(target)
            if parsed.scheme or parsed.netloc:
                continue
            destination = (path.parent / unquote(parsed.path)).resolve() if parsed.path else path
            if not destination.exists():
                failures.append(f"{relative}: missing link {target}")
            elif parsed.fragment and destination.suffix == ".md":
                if unquote(parsed.fragment) not in anchors(destination.read_text(encoding="utf-8")):
                    failures.append(f"{relative}: missing heading {target}")
        if len(re.findall(r"^```", text, re.M)) % 2:
            failures.append(f"{relative}: unclosed code fence")
    if failures:
        print("\n".join(failures), file=sys.stderr)
        return 1
    print(f"PASS: {len(files)} Markdown files; local links, headings, code fences and context budgets; active {active_bytes:,} bytes")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
