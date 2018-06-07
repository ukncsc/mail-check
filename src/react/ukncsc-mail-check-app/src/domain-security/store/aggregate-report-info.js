import { createAction, handleActions } from 'redux-actions';
import get from 'lodash/get';
import { mailCheckApiFetch } from 'common/helpers';
import moment from 'moment';

const aggregateReportInfoFetchRequest =
  'mailCheck/aggregateReportInfo/FETCH_REQUEST';
const aggregateReportInfoFetchSuccess =
  'mailCheck/aggregateReportInfo/FETCH_SUCCESS';
const aggregateReportInfoFetchError =
  'mailCheck/aggregateReportInfo/FETCH_ERROR';

export const fetchRequestAggregateReportInfo = createAction(
  aggregateReportInfoFetchRequest
);
export const fetchSuccessAggregateReportInfo = createAction(
  aggregateReportInfoFetchSuccess
);
export const fetchErrorAggregateReportInfo = createAction(
  aggregateReportInfoFetchError
);

export const fetchAggregateReportInfo = id => async dispatch => {
  try {
    dispatch(fetchRequestAggregateReportInfo({ id }));
    const response = await mailCheckApiFetch(
      `/domainstatus/domain/aggregate/${id}/${moment()
        .subtract(1, 'week')
        .format('YYYY-MM-DD')}/${moment()
        .subtract(1, 'day')
        .format('YYYY-MM-DD')}`
    );
    const result = response ? { id, data: { ...response } } : { id };
    dispatch(fetchSuccessAggregateReportInfo(result));
  } catch (err) {
    err.id = id;
    dispatch(fetchErrorAggregateReportInfo(err));
  }
};

const initialState = {};

export default handleActions(
  {
    [fetchRequestAggregateReportInfo]: (state, action) => ({
      ...state,
      [action.payload.id]: { loading: true },
    }),

    [fetchSuccessAggregateReportInfo]: (state, action) => ({
      ...state,
      [action.payload.id]: action.payload,
    }),

    [fetchErrorAggregateReportInfo]: (state, action) => ({
      ...state,
      [action.payload.id]: {
        error: action.payload.message,
      },
    }),
  },
  initialState
);

export const getAggregateReportInfo = state => id =>
  get(state, `domainSecurity.aggregateReport[${id}]`, {});
