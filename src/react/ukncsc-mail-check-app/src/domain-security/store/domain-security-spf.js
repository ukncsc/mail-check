import { createAction, handleActions } from 'redux-actions';
import get from 'lodash/get';
import reduce from 'lodash/reduce';
import { mailCheckApiFetch } from 'common/helpers';
import { recordsReducer, recordErrorsReducer } from './helpers';

const fetchRequestDomainSecuritySpfAction =
  'mailCheck/domainSecurity/FETCH_SPF_REQUEST';

const fetchSuccessDomainSecuritySpfAction =
  'mailCheck/domainSecurity/FETCH_SPF_SUCCESS';

const fetchErrorDomainSecuritySpfAction =
  'mailCheck/domainSecurity/FETCH_SPF_ERROR';

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

const transformResponse = ({ id, records, errors, lastChecked }) => ({
  id,
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

export const getDomainSecuritySpf = state => id =>
  get(state, `domainSecurity.spf[${id}]`, {});
