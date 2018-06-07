import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import rootReducer from './stores';

const initialState = {};
const enhancers = [];
const middleware = [thunk];

const { NODE_ENV: nodeEnv = 'development' } = process.env;
const devToolsExtension = window['devToolsExtension'];

if (nodeEnv === 'development' && typeof devToolsExtension === 'function') {
  enhancers.push(devToolsExtension());
}

const composedEnhancers = compose(applyMiddleware(...middleware), ...enhancers);

const store = createStore(rootReducer, initialState, composedEnhancers);

export default store;
