import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import { createBrowserHistory } from 'history';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import rootReducer from './reducers';
import initialState from './initial-state';

export const history = createBrowserHistory({
  basename: process.env.REACT_APP_URL_ROUTE || '',
});

const enhancers = [];
const middleware = [thunk, routerMiddleware(history)];
const { NODE_ENV: nodeEnv = 'development' } = process.env;
const { devToolsExtension } = window;

if (nodeEnv === 'development' && typeof devToolsExtension === 'function') {
  enhancers.push(devToolsExtension());
}

const composedEnhancers = compose(
  applyMiddleware(...middleware),
  ...enhancers
);

export default createStore(
  connectRouter(history)(rootReducer),
  initialState,
  composedEnhancers
);
