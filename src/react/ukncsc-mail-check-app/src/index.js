import React from 'react';
import 'react-dates/lib/css/_datepicker.css';
import 'react-dates/initialize';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';

import 'core-js/fn/array';
import 'core-js/fn/string';
import 'ukncsc-semantic-ui-theme';

import store, { history } from 'common/store';

import App from './App';
import { unregister } from './registerServiceWorker';

import './index.css';

render(
  <Provider store={store}>
    <ConnectedRouter history={history}>
      <App />
    </ConnectedRouter>
  </Provider>,
  document.getElementById('root')
);

unregister();
