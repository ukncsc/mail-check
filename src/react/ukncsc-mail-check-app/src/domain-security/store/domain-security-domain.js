import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestDomainSecurityDomainAction =
  'mailCheck/domainSecurity/domain/FETCH_REQUEST';

const fetchSuccessDomainSecurityDomainAction =
  'mailCheck/domainSecurity/domain/FETCH_SUCCESS';

const fetchErrorDomainSecurityDomainAction =
  'mailCheck/domainSecurity/domain/FETCH_ERROR';

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
  state.domainSecurity.domain[id];
