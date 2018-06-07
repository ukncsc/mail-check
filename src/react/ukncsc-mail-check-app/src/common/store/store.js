import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import rootReducer from './reducers';
import initialState from './initial-state';

const enhancers = [];
const middleware = [thunk];
const { NODE_ENV: nodeEnv = 'development' } = process.env;
const { devToolsExtension } = window;

if (nodeEnv === 'development' && typeof devToolsExtension === 'function') {
  enhancers.push(devToolsExtension());
}

const composedEnhancers = compose(applyMiddleware(...middleware), ...enhancers);

export default createStore(rootReducer, initialState, composedEnhancers);
