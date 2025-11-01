# Feature Plan: Export Actual Counts to Excel Using a Template

## Feature Summary

**Goal:**
Enable exporting the actual counts table (Centers × Sewa Types) to an Excel file, using a pre-defined Excel template stored on the server. The exported Excel will have Sewa Type names as column headers and Center names as row headers, with the corresponding counts filled in the correct cells.

**Key Points:**
- The export should use an existing Excel template (e.g., `.xlsx` file) on the server.
- The template will have a table structure with Sewa Types as columns and Centers as rows.
- The exported file will be filled with actual count data for the selected date.
- The user will be able to download the filled Excel file from the web UI.


## Template Structure Details (as provided)
- **Sewa Type Headers:** Start from cell **E3** and go till **Q3** (one column per Sewa Type).
- **Row 4:** Is a sub-header row and can be ignored for data population.
- **Center Names:** Start from cell **C5** (one row per Center, C5, C6, ...).
- **Serial Number:** Column **B** (B5, B6, ...), to be filled with serial numbers for each center row.
- **Center Type:** Column **D** (D5, D6, ...), to be filled with the center type for each center row.
- **Data Values:** Start from **E5** (E5, F5, ..., Q5 for the first center; E6, F6, ..., Q6 for the next, etc.).
- **Date:** Cell **C2** should be populated with the selected/exported date.

## Implementation Plan

### 1. **Template Preparation**
- Prepare an Excel template file (e.g., `actual_counts_template.xlsx`) with:
  - The first row as Sewa Type names (column headers).
  - The first column as Center names (row headers).
  - The rest of the cells left empty for data population.
- Save this template in a known location on the server (e.g., `wwwroot/templates/`).

### 2. **Backend: Excel Export Endpoint**
- Create a new controller action (e.g., `ExportActualCountsExcel`) in the relevant controller (`AllotedCountController` or similar).
- The action will:
  1. Receive the selected date as a parameter.
  2. Load the Excel template using a library like [EPPlus](https://github.com/JanKallman/EPPlus) or [ClosedXML](https://github.com/ClosedXML/ClosedXML).
  3. Query the actual counts for the selected date, grouped by Center and Sewa Type.
  4. For each Center (row) and Sewa Type (column), fill the corresponding cell (E5:Q5, E6:Q6, ...) with the actual count value.
  5. Fill serial numbers in column B, center names in column C, and center types in column D for each row.
  6. Fill the selected date in cell C2.
  7. Return the filled Excel file as a downloadable response (`application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`).

### 3. **Frontend: Add Export Button**
- Add an "Export Excel" button next to the existing CSV export button in the UI (`PendingActual.cshtml`).
- On click, trigger a GET request to the new backend endpoint, passing the selected date as a query parameter.
- The browser should download the generated Excel file.

### 4. **Testing**
- Test with various templates and data to ensure correct mapping and formatting.
- Validate that the exported Excel matches the template and contains the correct data.

### 5. **Documentation**
- Document the template format and any requirements for future template changes.

## Additional Notes
- The Excel template can include formatting, formulas, or branding as needed; only the data cells will be filled programmatically.
- The implementation will use a .NET Excel library (EPPlus or ClosedXML) for reading/writing `.xlsx` files.
- Proper error handling will be added for missing templates, data, or invalid requests.

---

**Summary of Understanding:**
- You want to export the same data as the CSV export, but into a pre-formatted Excel file.
- The Excel file should use a template, and the data should be filled into the correct cells based on Center and Sewa Type.
- The user should be able to trigger this export from the UI and download the resulting Excel file.
- The template structure is now clearly defined, so the backend logic must map data to the correct cells as per the template.
