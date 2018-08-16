import { createAction, handleActions } from 'redux-actions';
import get from 'lodash/get';
import reduce from 'lodash/reduce';
import { mailCheckApiFetch } from 'common/helpers';
import { recordErrorsReducer, recordsReducer } from './helpers';

const fetchRequestDomainSecurityDmarcAction =
  'mailCheck/domainSecurity/dmarc/FETCH_REQUEST';

const fetchSuccessDomainSecurityDmarcAction =
  'mailCheck/domainSecurity/dmarc/FETCH_SUCCESS';

const fetchErrorDomainSecurityDmarcAction =
  'mailCheck/domainSecurity/dmarc/FETCH_ERROR';

export const fetchRequestDomainSecurityDmarc = createAction(
  fetchRequestDomainSecurityDmarcAction
);

export const fetchSuccessDomainSecurityDmarc = createAction(
  fetchSuccessDomainSecurityDmarcAction
);

export const fetchErrorDomainSecurityDmarc = createAction(
  fetchErrorDomainSecurityDmarcAction
);

export const fetchDomainSecurityDmarc = id => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecurityDmarc({ id }));
    const response = await mailCheckApiFetch(
      `/domainstatus/domain/dmarc/${id}`
    );
    dispatch(fetchSuccessDomainSecurityDmarc({ id, ...response }));
  } catch (error) {
    dispatch(fetchErrorDomainSecurityDmarc({ id, error }));
  }
};

const initialState = {};

const transformResponse = ({
  id,
  pending,
  records,
  errors,
  inheritedFrom,
  lastChecked,
}) => ({
  id,
  pending: pending || false,
  records:
    records && reduce(records, recordsReducer('tags', '', inheritedFrom), []),
  ...reduce(errors, recordErrorsReducer, {
    warnings: [],
    failures: [],
    inconclusives: [],
  }),
  inheritedFrom,
  lastChecked,
});

export default handleActions(
  {
    [fetchRequestDomainSecurityDmarcAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { loading: true },
    }),

    [fetchSuccessDomainSecurityDmarcAction]: (state, action) => ({
      ...state,
      [action.payload.id]: transformResponse(action.payload),
    }),

    [fetchErrorDomainSecurityDmarcAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { error: action.payload.error },
    }),
  },
  initialState
);

export const getDomainSecurityDmarc = state => id =>
  get(state, `domainSecurity.dmarc[${id}]`);
