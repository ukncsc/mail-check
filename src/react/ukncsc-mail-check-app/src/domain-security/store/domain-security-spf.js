import { createAction, handleActions } from 'redux-actions';
import reduce from 'lodash/reduce';
import { mailCheckApiFetch } from 'common/helpers';
import { recordsReducer, recordErrorsReducer } from './helpers';

const fetchRequestDomainSecuritySpfAction =
  'mailCheck/domainSecurity/spf/FETCH_REQUEST';

const fetchSuccessDomainSecuritySpfAction =
  'mailCheck/domainSecurity/spf/FETCH_SUCCESS';

const fetchErrorDomainSecuritySpfAction =
  'mailCheck/domainSecurity/spf/FETCH_ERROR';

export const fetchRequestDomainSecuritySpf = createAction(
  fetchRequestDomainSecuritySpfAction
);

export const fetchSuccessDomainSecuritySpf = createAction(
  fetchSuccessDomainSecuritySpfAction
);
export const fetchErrorDomainSecuritySpf = createAction(
  fetchErrorDomainSecuritySpfAction
);

export const fetchDomainSecuritySpf = id => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecuritySpf({ id }));
    const response = await mailCheckApiFetch(`/domainstatus/domain/spf/${id}`);
    dispatch(fetchSuccessDomainSecuritySpf({ id, ...response }));
  } catch (error) {
    dispatch(fetchErrorDomainSecuritySpf({ id, error }));
  }
};

const initialState = {};

const transformResponse = ({ id, pending, records, errors, lastChecked }) => ({
  id,
  pending: pending || false,
  records: records && reduce(records, recordsReducer('terms'), []),
  ...reduce(errors, recordErrorsReducer, {
    warnings: [],
    failures: [],
    inconclusives: [],
  }),
  lastChecked,
});

export default handleActions(
  {
    [fetchRequestDomainSecuritySpfAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { loading: true },
    }),

    [fetchSuccessDomainSecuritySpfAction]: (state, action) => ({
      ...state,
      [action.payload.id]: transformResponse(action.payload),
    }),

    [fetchErrorDomainSecuritySpfAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { error: action.payload.error },
    }),
  },
  initialState
);

export const getDomainSecuritySpf = state => id => state.domainSecurity.spf[id];
