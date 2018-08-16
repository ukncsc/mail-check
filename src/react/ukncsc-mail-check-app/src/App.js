import React from 'react';
import { Container } from 'semantic-ui-react';
import { ScrollToTop } from 'common/components';
import { PageFooter } from 'page/components';
import { PageHeader } from 'page/containers';
import PageRoutes from 'page/routes';

import './App.css';

export default () => (
  <div className="App">
    <PageHeader />
    <ScrollToTop>
      <Container className="App--content">
        <PageRoutes />
      </Container>
    </ScrollToTop>
    <PageFooter />
  </div>
);
