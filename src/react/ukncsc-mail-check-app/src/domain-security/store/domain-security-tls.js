import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestDomainSecurityTlsAction =
  'mailCheck/domainSecurity/tls/FETCH_REQUEST';

const fetchSuccessDomainSecurityTlsAction =
  'mailCheck/domainSecurity/tls/FETCH_SUCCESS';

const fetchErrorDomainSecurityTlsAction =
  'mailCheck/domainSecurity/tls/FETCH_ERROR';

export const fetchRequestDomainSecurityTls = createAction(
  fetchRequestDomainSecurityTlsAction
);

export const fetchSuccessDomainSecurityTls = createAction(
  fetchSuccessDomainSecurityTlsAction
);

export const fetchErrorDomainSecurityTls = createAction(
  fetchErrorDomainSecurityTlsAction
);

export const fetchDomainSecurityTls = id => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecurityTls({ id }));
    const response = await mailCheckApiFetch(`/domainstatus/domain/tls/${id}`);
    dispatch(fetchSuccessDomainSecurityTls(response));
  } catch (error) {
    dispatch(fetchErrorDomainSecurityTls({ id, error }));
  }
};

const initialState = {};

const transformResponse = ({ mxTlsEvaluatorResults: records, ...rest }) => ({
  ...rest,
  records,
});

export default handleActions(
  {
    [fetchRequestDomainSecurityTlsAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { loading: true },
    }),

    [fetchSuccessDomainSecurityTlsAction]: (state, action) => ({
      ...state,
      [action.payload.id]: transformResponse(action.payload),
    }),

    [fetchErrorDomainSecurityTlsAction]: (state, action) => ({
      ...state,
      [action.payload.id]: { error: action.payload.error },
    }),
  },
  initialState
);

export const getDomainSecurityTls = state => id => state.domainSecurity.tls[id];
