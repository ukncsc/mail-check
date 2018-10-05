import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestDomainSecurityDkimAction =
  'mailCheck/domainSecurity/dkim/FETCH_REQUEST';

const fetchSuccessDomainSecurityDkimAction =
  'mailCheck/domainSecurity/dkim/FETCH_SUCCESS';

const fetchErrorDomainSecurityDkimAction =
  'mailCheck/domainSecurity/dkim/FETCH_ERROR';

export const fetchRequestDomainSecurityDkim = createAction(
  fetchRequestDomainSecurityDkimAction
);
export const fetchSuccessDomainSecurityDkim = createAction(
  fetchSuccessDomainSecurityDkimAction
);
export const fetchErrorDomainSecurityDkim = createAction(
  fetchErrorDomainSecurityDkimAction
);

const transformResponse = ({ domain: domainName, selectors }) => ({
  domainName,
  records: selectors.map(({ selector, records, messages }) => ({
    hostname: selector,
    failures: messages.filter(_ => _.severity === 'Error').map(_ => _.message),
    warnings: messages
      .filter(_ => _.severity === 'Warning')
      .map(_ => _.message),
    inconclusives: messages
      .filter(_ => _.severity === 'Inconclusive')
      .map(_ => _.message),
    records,
  })),
  pending: !selectors.length,
});

export const fetchDomainSecurityDkim = domainName => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecurityDkim({ domainName }));
    const response = await mailCheckApiFetch(`/dkim/domain/${domainName}`);
    dispatch(fetchSuccessDomainSecurityDkim(response));
  } catch ({ message, stack }) {
    dispatch(
      fetchErrorDomainSecurityDkim({
        domainName,
        error: { message, stack },
      })
    );
  }
};

const initialState = {};

export default handleActions(
  {
    [fetchRequestDomainSecurityDkimAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: {
        ...state[action.payload.domainName],
        error: null,
        loading: true,
      },
    }),

    [fetchSuccessDomainSecurityDkimAction]: (state, action) => ({
      ...state,
      [action.payload.domain]: {
        ...state[action.payload.domain],
        ...transformResponse(action.payload),
        error: null,
        loading: false,
      },
    }),

    [fetchErrorDomainSecurityDkimAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: {
        ...state[action.payload.domainName],
        error: action.payload.error,
        loading: false,
      },
    }),
  },
  initialState
);

export const getDomainSecurityDkim = state => domainName =>
  state.domainSecurity.dkim[domainName];
