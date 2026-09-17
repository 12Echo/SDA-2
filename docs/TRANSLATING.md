# Translating Steam Desktop Authenticator 2

Translations live in [`languages/`](../languages) as json files. The app loads
`languages/<code>.json` next to the executable at startup when that code is selected in
Settings, and replaces every piece of text it can find with the value from the file.

## File format

```json
{
  "_name": "Deutsch",
  "_author": "your name or handle",
  "Settings": "Einstellungen",
  "Copy": "Kopieren",
  "Nothing to confirm": "Nichts zu bestätigen"
}
```

* Keys are the exact English text. `template.json` lists all of them.
* Values are the translation. An empty value means "keep English".
* `_name` is what shows in the language picker. `_author` is optional.
* `\n` inside a value is a line break, keep them where the English has them.

## How text is replaced

* Every control on every form (labels, buttons, check boxes, menus, window titles,
  placeholder text) is translated after the form is built.
* Message boxes translate their text, caption and button labels.
* Text built from pieces at runtime, such as `"Session expires in " + days + " days"`,
  is not covered yet. Those are the strings you will still see in English.

## Sending a translation

Open a pull request that adds `languages/<code>.json`. Use the language codes from
[IETF BCP 47](https://en.wikipedia.org/wiki/IETF_language_tag), for example `de`,
`fr`, `pt-BR`, `ru`, `zh-Hans`. One language per pull request keeps review easy.

Before sending it, start SDA with the language selected and click through the settings,
the confirmations window, the login window and at least one message box. Long
translations can overflow a control, say so in the pull request if you spot one and the
layout will be adjusted.
