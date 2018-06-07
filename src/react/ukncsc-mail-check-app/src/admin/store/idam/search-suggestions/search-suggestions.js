import { createAction, handleActions } from 'redux-actions';
import map from 'lodash/map';
import mapValues from 'lodash/mapValues';
import { mailCheckApiFetch } from 'common/helpers';

const updateIdamSearchTermAction =
  'mailCheck/admin/idam/searchSuggestions/term/UPDATE';

const fetchRequestIdamSearchSuggestionsAction =
  'mailCheck/admin/idam/searchSuggestions/FETCH_REQUEST';

export const fetchSuccessIdamSearchSuggestionsAction =
  'mailCheck/admin/idam/searchSuggestions/FETCH_SUCCESS';

const fetchErrorIdamSearchSuggestionsAction =
  'mailCheck/admin/idam/searchSuggestions/FETCH_ERROR';

export const updateIdamSearchTerm = createAction(updateIdamSearchTermAction);

export const fetchRequestIdamSearchSuggestions = createAction(
  fetchRequestIdamSearchSuggestionsAction
);

export const fetchSuccessIdamSearchSuggestions = createAction(
  fetchSuccessIdamSearchSuggestionsAction
);

export const fetchErrorIdamSearchSuggestions = createAction(
  fetchErrorIdamSearchSuggestionsAction
);

export const fetchIdamSearchSuggestions = (
  term,
  limit = 10
) => async dispatch => {
  try {
    dispatch(fetchRequestIdamSearchSuggestions());
    const response = await mailCheckApiFetch(
      `/admin/search/${term}?limit=${limit}`
    );
    dispatch(fetchSuccessIdamSearchSuggestions(response));
  } catch (err) {
    dispatch(fetchErrorIdamSearchSuggestions(err));
  }
};

const initialState = {
  term: '',
  results: {},
  isLoading: false,
};

export default handleActions(
  {
    [updateIdamSearchTermAction]: (state, action) => ({
      ...state,
      term: action.payload,
    }),

    [fetchRequestIdamSearchSuggestionsAction]: state => ({
      ...state,
      isLoading: true,
    }),

    [fetchSuccessIdamSearchSuggestionsAction]: (state, action) => ({
      ...state,
      results: mapValues(action.payload, ({ name, results }, entityType) => ({
        name,
        results: map(results, ({ id: entityId }) => ({ entityId, entityType })),
      })),
      isLoading: false,
    }),

    [fetchErrorIdamSearchSuggestionsAction]: (state, action) => ({
      ...state,
      isLoading: false,
      error: action.payload.message,
    }),
  },
  initialState
);

export const getIdamSearchSuggestions = state =>
  state.admin.idam.searchSuggestions;

export const getIdamSearchSuggestionsTerm = state =>
  state.admin.idam.searchSuggestions.term;
