import { createAction, handleActions } from 'redux-actions';
import get from 'lodash/get';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestDomainSecurityDomainAction =
  'mailCheck/domainSecurity/FETCH_DOMAIN_REQUEST';

const fetchSuccessDomainSecurityDomainAction =
  'mailCheck/domainSecurity/FETCH_DOMAIN_SUCCESS';

const fetchErrorDomainSecurityDomainAction =
  'mailCheck/domainSecurity/FETCH_DOMAIN_ERROR';

export const fetchRequestDomainSecurityDomain = createAction(
  fetchRequestDomainSecurityDomainAction
);

export const fetchSuccessDomainSecurityDomain = createAction(
  fetchSuccessDomainSecurityDomainAction
);

export const fetchErrorDomainSecurityDomain = createAction(
  fetchErrorDomainSecurityDomainAction
);

export const fetchDomainSecurityDomain = id => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecurityDomain({ id }));
    const response = await mailCheckApiFetch(`/domainstatus/domain/${id}`);
    dispatch(fetchSuccessDomainSecurityDomain(response));
  } catch (error) {
    dispatch(fetchErrorDomainSecurityDomain({ id, error }));
  }
};

const initialState = {};

export default handleActions(
  {
    [fetchRequestDomainSecurityDomainAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { loading: true },
    }),

    [fetchSuccessDomainSecurityDomainAction]: (state, action) => ({
      ...state,
      [action.payload.id]: action.payload,
    }),

    [fetchErrorDomainSecurityDomainAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { error: action.payload.error },
    }),
  },
  initialState
);

export const getDomainSecurityDomain = state => id =>
  get(state, `domainSecurity.domain[${id}]`, {});
