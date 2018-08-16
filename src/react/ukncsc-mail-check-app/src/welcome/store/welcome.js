import { push } from 'connected-react-router';
import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestAction = 'mailCheck/welcome/search/FETCH_REQUEST';

const fetchSuccessAction = 'mailCheck/welcome/search/FETCH_SUCCESS';

const fetchErrorAction = 'mailCheck/welcome/search/FETCH_ERROR';

const resetWelcomeSearchAction = 'mailCheck/welcome/search/RESET';

const updateWelcomeSearchAction = 'mailCheck/welcome/search/UPDATE';

export const fetchRequestWelcomeSearch = createAction(fetchRequestAction);

export const fetchSuccessWelcomeSearch = createAction(fetchSuccessAction);

export const fetchErrorWelcomeSearch = createAction(fetchErrorAction);

export const resetWelcomeSearch = createAction(resetWelcomeSearchAction);

export const updateWelcomeSearch = createAction(updateWelcomeSearchAction);

export const fetchWelcomeSearch = searchTerm => async dispatch => {
  dispatch(updateWelcomeSearch({ lastSearchTerm: searchTerm }));

  try {
    dispatch(fetchRequestWelcomeSearch());
    const response = await mailCheckApiFetch(
      `domainstatus/welcome?search=${searchTerm}`
    );
    dispatch(fetchSuccessWelcomeSearch(response));
  } catch ({ message }) {
    dispatch(fetchErrorWelcomeSearch({ message }));
  }
};

export const fetchAddDomainToMailCheck = name => async dispatch => {
  try {
    dispatch(fetchRequestWelcomeSearch());
    const response = await mailCheckApiFetch('admin/domain', 'POST', {
      name,
    });
    dispatch(push(`/domain-security/${response.id}`));
  } catch ({ message }) {
    dispatch(fetchErrorWelcomeSearch({ message }));
  }
};

const initialState = {
  error: null,
  hasSearched: false,
  lastSearchTerm: '',
  loading: false,
  searchResult: null,
  searchTerm: '',
  searchTermIsPublicSectorOrg: false,
};

export default handleActions(
  {
    [fetchRequestAction]: state => ({
      ...state,
      loading: true,
      error: null,
    }),

    [fetchSuccessAction]: (state, action) => ({
      ...state,
      ...action.payload,
      error: null,
      hasSearched: true,
      loading: false,
    }),

    [fetchErrorAction]: (state, action) => ({
      ...state,
      error: action.payload,
      hasSearched: true,
      loading: false,
    }),

    [resetWelcomeSearchAction]: () => initialState,

    [updateWelcomeSearchAction]: (state, action) => ({
      ...state,
      ...action.payload,
    }),
  },
  initialState
);

export const getWelcomeSearch = state => state.welcome;
