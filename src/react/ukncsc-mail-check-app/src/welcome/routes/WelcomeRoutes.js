import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { WelcomeContainer } from 'welcome/containers';

export default () => (
  <Switch>
    <Route exact path="/home/:term?" component={WelcomeContainer} />
  </Switch>
);
