# Execution Plan: Export Actual Counts to Excel Using a Template

## Objective
Implement the feature to export actual counts to Excel using a server-side template, as described in the feature plan and template mapping.

## Step-by-Step Execution Plan

### 1. **Template File Preparation**
- [ ] Ensure the Excel template (`actual_counts_template.xlsx`) is created and placed in `wwwroot/templates/`.
- [ ] Verify the template structure matches the mapping:
  - Sewa Type headers: E3–Q3
  - Center names: C5 down
  - Serial number: B5 down
  - Center type: D5 down
  - Data values: E5 (and right/down)
  - Date: C2

### 2. **Install Excel Library**
- [ ] Add EPPlus or ClosedXML NuGet package to the project for Excel manipulation.

### 3. **Backend Implementation**
- [ ] Add a new endpoint (e.g., `ExportActualCountsExcel`) in the appropriate controller.
- [ ] In the endpoint:
  - [ ] Accept the selected date as a parameter.
  - [ ] Load the Excel template from disk.
  - [ ] Query the actual counts for the selected date, grouped by Center and Sewa Type.
  - [ ] For each center row:
    - [ ] Fill serial number (B), center name (C), center type (D).
    - [ ] For each Sewa Type, fill the actual count in the correct cell (E–Q).
  - [ ] Fill the selected date in cell C2.
  - [ ] Return the filled Excel file as a downloadable response.
- [ ] Add error handling for missing template, missing data, or invalid requests.

### 4. **Frontend Integration**
- [ ] Add an "Export Excel" button to `PendingActual.cshtml` next to the CSV export button.
- [ ] On click, trigger a GET request to the backend endpoint with the selected date.
- [ ] Ensure the browser downloads the generated Excel file.

### 5. **Testing & Validation**
- [ ] Test with various dates and data to ensure correct mapping and output.
- [ ] Validate that the Excel file matches the template and contains correct data.
- [ ] Test error scenarios (missing template, no data, etc.).

### 6. **Documentation**
- [ ] Update documentation to describe the template format, endpoint usage, and any requirements for future changes.

---

**Summary:**
This execution plan provides a checklist for implementing the Excel export feature, covering template setup, backend logic, frontend integration, and testing. Each step should be checked off as completed to ensure a smooth and complete implementation.
