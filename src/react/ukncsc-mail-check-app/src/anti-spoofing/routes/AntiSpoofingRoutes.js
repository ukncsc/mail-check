import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { AntiSpoofing } from 'anti-spoofing/containers';

export default () => (
  <Switch>
    <Route exact path="/domain-security" component={AntiSpoofing} />
  </Switch>
);
