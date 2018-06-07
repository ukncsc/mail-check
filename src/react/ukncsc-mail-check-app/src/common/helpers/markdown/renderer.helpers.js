import React from 'react';

// eslint-disable-next-line import/prefer-default-export
export const removeMailtoHyperlinksRenderer = _ =>
  _.href.startsWith('mailto:') ? _.href : <a href={_.href}>{_.children}</a>;
