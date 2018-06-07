import React from 'react';
import 'react-dates/lib/css/_datepicker.css';
import 'react-dates/initialize';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';

import 'core-js/fn/array/from';
import 'ukncsc-semantic-ui-theme';

import store from 'common/store';

import App from './App';
import { unregister } from './registerServiceWorker';

import './index.css';

render(
  <Provider store={store}>
    <BrowserRouter basename={process.env.REACT_APP_URL_ROUTE || ''}>
      <App />
    </BrowserRouter>
  </Provider>,
  document.getElementById('root')
);

unregister();
