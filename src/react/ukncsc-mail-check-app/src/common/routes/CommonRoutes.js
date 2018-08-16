import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import { TermsAndConditions } from 'common/components';

export default () => (
  <Switch>
    <Route exact path="/terms" component={TermsAndConditions} />
    <Route exact path="/" component={() => <Redirect from="/" to="/home" />} />
  </Switch>
);
