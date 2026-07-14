# Components / Inputs

Input controls exposed by the library:

- `TextInput`
- `TextArea`
- `NumberInput<TNumber>`
- `DatePicker`
- `Dropdown<TItem, TValue>` (single-select)
- `MultiSelectDropdown<TItem, TValue>`
- `FileUpload` (drag-and-drop enabled)

Each derives from `BaseInputComponent<T>` and supports label, placeholder, disabled, `@bind-Value`, validation, events, custom CSS, and error display.
