# UK NCSC Semantic UI Theme

## Introduction

This repository provides the UK National Cyber Security Centre theme for
Semantic UI.

## Usage

If you would like to use this module as-is, you can simply `import` or `require`
it in your project (with an appropriate build configuration – see
[css-loader](https://github.com/webpack-contrib/css-loader) for inspiration). If
you just want to use it statically, then add the minified CSS file as a `link`
in your HTML:

```js
import 'ukncsc-semantic-ui-theme';
// or
require('ukncsc-semantic-ui-theme');
```

```html
<link src="/path/to/semantic.min.css"/>
```

## Development

### Prerequisites

* [Node.js](https://nodejs.org/en/) version 6 or higher
* [Yarn](https://yarnpkg.com/lang/en/) (NPM can be used, but the examples here
  will be using Yarn)

### Working Locally

To make changes to the theme and consume it in another project locally, you can
run `yarn link` in order to make it available globally on your machine. Then in
the other project directory, run `yarn link ukncsc-semantic-ui-theme` and the
module will be available for use. Using the `start` script can be useful for
this situation as changes to the theme are available in the other project almost
instantly.

### Scripts

#### `yarn build`

Creates a production build of the theme.

#### `yarn start`

Watches files in `src` and builds on changes.
