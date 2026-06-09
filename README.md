# PDF Updater Tool

A C# .NET 9 console utility designed to modify and update text in existing PDF quotation files. It preserves the original document layout, fonts, colors, and styling by precisely overlaying white masking boxes and redrawing text using standard and system fonts at exact coordinate baselines.

## Features

1. **Page 1 (Metadata Table):**
   * Replaces `"Date:"` with `"Date of Issue:"`.
   * Calculates string width using `Helvetica 8.5pt` to right-align the new label perfectly with the `"Quote No:"` label.
2. **Page 4 (Terms & Conditions):**
   * Replaces the English and Chinese payment terms text under Section 5.
   * Cleans up mispositioned text at the bottom.
   * Renders the updated text block exactly at the original coordinates using `Arial Regular`, `Arial Bold` (for headers), and `Microsoft YaHei` (for Chinese characters).

---

## Requirements

* **.NET 9.0 SDK** or higher.
* **Windows OS** (uses system fonts `Arial` and `Microsoft YaHei` located in `C:\Windows\Fonts`).

---

## Setup & Running

1. **Clone the Repository:**
   ```bash
   git clone <repository-url>
   cd pdf-updater-tool
   ```

2. **Run the Application:**
   By default, it will look for the source PDF in the parent directory.
   ```bash
   dotnet run
   ```

   Alternatively, you can pass custom file paths:
   ```bash
   dotnet run -- "path/to/source.pdf" "path/to/destination.pdf"
   ```
