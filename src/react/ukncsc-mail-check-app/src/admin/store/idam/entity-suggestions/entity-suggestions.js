import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';
import { getSearchUrl } from './entity-suggestions.helpers';

const updateIdamEntityTermAction =
  'mailCheck/admin/idam/entitySuggestions/term/UPDATE';

const updateIdamEntitySuggestionTypeAction =
  'mailCheck/admin/idam/entitySuggestions/type/UPDATE';

const updateIdamEntitySuggestionSelectedAction =
  'mailCheck/admin/idam/entitySuggestions/selected/UPDATE';

const resetIdamEntitySelectionAction =
  'mailCheck/admin/idam/entitySuggestions/RESET';

const fetchRequestIdamEntitySuggestionsAction =
  'mailCheck/admin/idam/entitySuggestions/FETCH_REQUEST';

export const fetchSuccessIdamEntitySuggestionsAction =
  'mailCheck/admin/idam/enititySuggestions/FETCH_SUCCESS';

const fetchErrorIdamEntitySuggestionsAction =
  'mailCheck/admin/idam/enititySuggestions/FETCH_ERROR';

export const updateIdamEntityTerm = createAction(updateIdamEntityTermAction);

export const updateIdamEntitySuggestionType = createAction(
  updateIdamEntitySuggestionTypeAction
);

export const updateIdamEntitySuggesionSelected = createAction(
  updateIdamEntitySuggestionSelectedAction
);

export const resetIdamEntitySelection = createAction(
  resetIdamEntitySelectionAction
);

export const fetchRequestIdamEntitySuggestions = createAction(
  fetchRequestIdamEntitySuggestionsAction
);

export const fetchSuccessIdamEntitySuggestions = createAction(
  fetchSuccessIdamEntitySuggestionsAction
);

export const fetchErrorIdamEntitySuggestions = createAction(
  fetchErrorIdamEntitySuggestionsAction
);

export const fetchIdamEntitySuggestions = (
  type,
  selected = [],
  term = '',
  limit = 10
) => async dispatch => {
  try {
    dispatch(fetchRequestIdamEntitySuggestions());
    const response = await mailCheckApiFetch(
      getSearchUrl(type, selected, term, limit)
    );
    dispatch(fetchSuccessIdamEntitySuggestions(response));
  } catch (err) {
    dispatch(fetchErrorIdamEntitySuggestions(err));
  }
};

const initialState = {
  term: '',
  type: '',
  results: [],
  selected: [],
  isLoading: false,
};

export default handleActions(
  {
    [updateIdamEntityTermAction]: (state, action) => ({
      ...state,
      term: action.payload,
    }),

    [updateIdamEntitySuggestionSelectedAction]: (state, action) => ({
      ...state,
      selected: action.payload,
    }),

    [updateIdamEntitySuggestionTypeAction]: (state, action) => ({
      ...state,
      type: action.payload,
    }),

    [resetIdamEntitySelectionAction]: () => initialState,

    [fetchRequestIdamEntitySuggestionsAction]: state => ({
      ...state,
      isLoading: true,
    }),

    [fetchSuccessIdamEntitySuggestionsAction]: (state, action) => ({
      ...state,
      results: action.payload,
      isLoading: false,
    }),

    [fetchErrorIdamEntitySuggestionsAction]: (state, action) => ({
      ...state,
      error: action.payload.message,
      isLoading: false,
    }),
  },
  initialState
);

export const getIdamEntitySuggestions = state =>
  state.admin.idam.entitySuggestions;
