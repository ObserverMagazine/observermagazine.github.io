#!/usr/bin/env bash
# ===========================================================================
# export.sh — Export git-tracked project files to docs/llm/dump.txt
#
# Blog posts (content/blog/*.md): only front matter is included; the
# markdown body is omitted to save context space. Every other git-tracked
# file is exported in full.
#
# Excluded from export:
#   - The docs/ directory itself (to avoid recursion / stale LLM context)
#   - Binary files (images, fonts, etc.)
#
# Usage:
#   bash export.sh            # writes docs/llm/dump.txt
#   bash export.sh -o out.txt # writes to a custom path
# ===========================================================================

set -euo pipefail

# ── Configuration ────────────────────────────────────────────────────────────

OUTPUT_FILE="docs/llm/dump.txt"
BLOG_DIR="content/blog"
SEPARATOR="-------------------------------------------------------------------------------"
WIDE_SEP="==============================================================================="

# Binary file extensions to skip (lowercase comparison)
BINARY_EXTENSIONS="png jpg jpeg gif ico bmp svg webp woff woff2 ttf eot otf mp3 mp4 wav ogg pdf zip tar gz"

# ── Parse arguments ──────────────────────────────────────────────────────────

while [[ $# -gt 0 ]]; do
    case "$1" in
        -o|--output)
            OUTPUT_FILE="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: bash export.sh [-o OUTPUT_FILE]"
            echo "  Exports git-tracked project files to docs/llm/dump.txt"
            echo "  Blog posts include front matter only (body omitted)."
            exit 0
            ;;
        *)
            echo "Unknown argument: $1" >&2
            exit 1
            ;;
    esac
done

# ── Helpers ──────────────────────────────────────────────────────────────────

is_binary_extension() {
    local file="$1"
    local ext="${file##*.}"
    ext="${ext,,}" # lowercase
    for bin_ext in $BINARY_EXTENSIONS; do
        if [[ "$ext" == "$bin_ext" ]]; then
            return 0
        fi
    done
    return 1
}

is_blog_post() {
    local file="$1"
    [[ "$file" == ${BLOG_DIR}/*.md ]]
}

is_excluded_dir() {
    local file="$1"
    [[ "$file" == docs/* ]]
}

# Extract only the YAML front matter (between first --- and second ---).
# Prints the front matter block including delimiters.
extract_front_matter() {
    local file="$1"
    local in_front_matter=0
    local found_start=0

    while IFS= read -r line; do
        if [[ "$found_start" -eq 0 && "$line" == "---" ]]; then
            found_start=1
            in_front_matter=1
            echo "$line"
            continue
        fi
        if [[ "$in_front_matter" -eq 1 && "$line" == "---" ]]; then
            echo "$line"
            return
        fi
        if [[ "$in_front_matter" -eq 1 ]]; then
            echo "$line"
        fi
    done < "$file"
}

# ── Collect git-tracked files ────────────────────────────────────────────────

mapfile -t ALL_FILES < <(git ls-files --cached --others --exclude-standard | sort)

# ── Counters ─────────────────────────────────────────────────────────────────

total_files=0
blog_front_matter_only=0
full_content_files=0
binary_skipped=0

# ── Ensure output directory exists ───────────────────────────────────────────

mkdir -p "$(dirname "$OUTPUT_FILE")"

# ── Write output ─────────────────────────────────────────────────────────────

{
    # Header
    echo "$WIDE_SEP"
    echo "PROJECT EXPORT: observermagazine.github.io"
    echo "DATE: $(date -u '+%Y-%m-%d %H:%M:%S UTC')"
    echo "$WIDE_SEP"
    echo ""

    # Directory tree
    echo "$WIDE_SEP"
    echo "DIRECTORY TREE (all git-tracked files)"
    echo "$WIDE_SEP"
    echo ""

    # Use tree if available, otherwise fall back to a simple listing
    if command -v tree &>/dev/null; then
        # Build a temp file list for tree --fromfile
        printf '%s\n' "${ALL_FILES[@]}" | tree --fromfile --charset utf-8 -a 2>/dev/null || {
            # Fallback: just print the sorted file list as an indented tree
            echo "."
            printf '%s\n' "${ALL_FILES[@]}" | sed 's|^|├── |'
        }
    else
        echo "."
        printf '%s\n' "${ALL_FILES[@]}" | sed 's|^|├── |'
    fi

    echo ""

    # File contents
    for file in "${ALL_FILES[@]}"; do
        # Skip files in docs/ directory
        if is_excluded_dir "$file"; then
            continue
        fi

        # Skip binary files
        if is_binary_extension "$file"; then
            binary_skipped=$((binary_skipped + 1))
            total_files=$((total_files + 1))
            continue
        fi

        # Skip the export script itself
        if [[ "$file" == "export.sh" ]]; then
            # Still include it — it's useful context for the LLM
            :
        fi

        echo ""
        echo "$SEPARATOR"

        if is_blog_post "$file"; then
            # Blog post: front matter only
            echo "FILE: ${file}  [FRONT MATTER ONLY — body omitted to save context space]"
            echo "$SEPARATOR"
            extract_front_matter "$file"
            echo ""
            blog_front_matter_only=$((blog_front_matter_only + 1))
        else
            # All other files: full content
            echo "FILE: ${file}"
            echo "$SEPARATOR"
            cat "$file"
            echo ""
            full_content_files=$((full_content_files + 1))
        fi

        total_files=$((total_files + 1))
    done

    # Footer
    echo ""
    echo "$WIDE_SEP"
    echo "EXPORT COMPLETED"
    echo "Total Files Exported: ${total_files}"
    echo "  Blog posts (front matter only): ${blog_front_matter_only}"
    echo "  Full content files: ${full_content_files}"
    echo "  Binary files skipped: ${binary_skipped}"
    echo "Output File: ${OUTPUT_FILE}"
    echo "$WIDE_SEP"

} > "$OUTPUT_FILE"

echo "Export complete: ${OUTPUT_FILE}"
echo "  Total files: ${total_files}"
echo "  Blog posts (front matter only): ${blog_front_matter_only}"
echo "  Full content files: ${full_content_files}"
echo "  Binary files skipped: ${binary_skipped}"
