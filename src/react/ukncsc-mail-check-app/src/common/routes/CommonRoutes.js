import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { TermsAndConditions } from 'common/components';
import { InitialRedirectContainer } from 'page/containers';

export default () => (
  <Switch>
    <Route exact path="/terms" component={TermsAndConditions} />
    <Route exact path="/" component={InitialRedirectContainer} />
  </Switch>
);
