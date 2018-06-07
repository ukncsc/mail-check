import React from 'react';
import { PageFooter, RouterOutput } from 'page/components';
import { PageHeader } from 'page/containers';

import './App.css';

export default () => (
  <div className="App">
    <PageHeader />
    <RouterOutput />
    <PageFooter />
  </div>
);
