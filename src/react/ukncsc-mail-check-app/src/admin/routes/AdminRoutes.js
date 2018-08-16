import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Admin } from 'admin/containers';

export default () => (
  <Switch>
    <Route exact path="/admin" component={Admin} />
  </Switch>
);
