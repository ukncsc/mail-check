import { createAction, handleActions } from 'redux-actions';
import {
  createFetchAction,
  createResourceReducers,
  createResourceInitialState,
  mailCheckApiFetch,
} from 'common/helpers';

const metricsFetchRequest = 'mailCheck/metrics/FETCH_REQUEST';
const metricsFetchSuccess = 'mailCheck/metrics/FETCH_SUCCESS';
const metricsFetchError = 'mailCheck/metrics/FETCH_ERROR';
export const fetchRequestMetrics = createAction(metricsFetchRequest);
export const fetchSuccessMetrics = createAction(metricsFetchSuccess);
export const fetchErrorMetrics = createAction(metricsFetchError);
export const fetchMetrics = createFetchAction(
  fetchRequestMetrics,
  fetchSuccessMetrics,
  fetchErrorMetrics,
  (start, end) => mailCheckApiFetch(`/metrics/${start}/${end}`)
);
export default handleActions(
  createResourceReducers(
    metricsFetchRequest,
    metricsFetchSuccess,
    metricsFetchError,
    'data'
  ),
  createResourceInitialState()
);
export const getMetrics = state => state.metrics;
