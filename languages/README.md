# Translations

Each file in this folder is one language. The file name is the language code
(`de.json`, `pt-BR.json`, `zh-Hans.json`) and it is picked in Settings.

To add a language:

1. Copy `template.json` to `<code>.json`.
2. Set `_name` to the language name as it should appear in Settings, in that language.
3. Fill in the values. The keys are the English text, the values are your translation.
   Leave a value empty to keep the English text for that entry.
4. Keep placeholders such as `{0}` and line breaks (`\n`) where they are.
5. Start SDA, choose the language in Settings, restart, and read through every screen.

`template.json` is regenerated for each release, so a translation may be missing keys
that were added later. Missing keys fall back to English, nothing breaks.

Text that is built at runtime, for example messages that include an account name or a
version number, is not translatable yet.
