import React from 'react';
import { Route } from 'react-router-dom';
import { Metrics } from 'metrics/containers';

export default () => <Route exact path="/metrics" component={Metrics} />;
