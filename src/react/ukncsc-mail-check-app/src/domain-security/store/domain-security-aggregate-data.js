import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';
import moment from 'moment';

const fetchRequestDomainSecurityAggregateDataAction =
  'mailCheck/domainSecurity/aggregateData/FETCH_REQUEST';

const fetchSuccessDomainSecurityAggregateDataAction =
  'mailCheck/domainSecurity/aggregateData/FETCH_SUCCESS';

const fetchErrorDomainSecurityAggregateDataAction =
  'mailCheck/domainSecurity/aggregateData/FETCH_ERROR';

export const fetchRequestDomainSecurityAggregateData = createAction(
  fetchRequestDomainSecurityAggregateDataAction
);
export const fetchSuccessDomainSecurityAggregateData = createAction(
  fetchSuccessDomainSecurityAggregateDataAction
);
export const fetchErrorDomainSecurityAggregateData = createAction(
  fetchErrorDomainSecurityAggregateDataAction
);

export const fetchDomainSecurityAggregateData = (
  id,
  days = 7,
  includeSubdomains = true
) => async dispatch => {
  try {
    dispatch(
      fetchRequestDomainSecurityAggregateData({
        id,
        selectedDays: days,
        includeSubdomains,
      })
    );
    const data = await mailCheckApiFetch(
      `/domainstatus/domain/aggregate/${id}/${moment()
        .subtract(days, 'days')
        .format('YYYY-MM-DD')}/${moment()
        .subtract(1, 'day')
        .format('YYYY-MM-DD')}?includeSubdomains=${includeSubdomains}`
    );
    dispatch(fetchSuccessDomainSecurityAggregateData({ id, data }));
  } catch (err) {
    err.id = id;
    dispatch(fetchErrorDomainSecurityAggregateData(err));
  }
};

const initialState = {};

export default handleActions(
  {
    [fetchRequestDomainSecurityAggregateData]: (state, action) => ({
      ...state,
      [action.payload.id]: {
        ...state[action.payload.id],
        ...action.payload,
        loading: true,
        error: null,
      },
    }),

    [fetchSuccessDomainSecurityAggregateData]: (state, action) => ({
      ...state,
      [action.payload.id]: {
        ...state[action.payload.id],
        ...action.payload,
        loading: false,
        error: null,
      },
    }),

    [fetchErrorDomainSecurityAggregateData]: (state, action) => ({
      ...state,
      [action.payload.id]: {
        ...state[action.payload.id],
        loading: false,
        error: action.payload,
      },
    }),
  },
  initialState
);

export const getDomainSecurityAggregateData = state => id =>
  state.domainSecurity.aggregateData[id];
