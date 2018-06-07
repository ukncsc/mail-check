import map from 'lodash/map';
import includes from 'lodash/includes';
import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const selectIdamCurrentEntityAction =
  'mailCheck/admin/idam/currentEntity/SELECTED';

const fetchRequestEntityAction =
  'mailCheck/admin/idam/currentEntity/ENTITY_FETCH_REQUEST';

export const fetchSuccessEntityAction =
  'mailCheck/admin/idam/currentEntity/ENTITY_FETCH_SUCCESS';

const fetchErrorEntityAction =
  'mailCheck/admin/idam/currentEntity/ENTITY_FETCH_ERROR';

const fetchRequestDeleteFromEntityAction =
  'mailCheck/admin/idam/currentEntity/DELETE_FROM_ENTITY_FETCH_REQUEST';

const fetchSuccessDeleteFromEntityAction =
  'mailCheck/admin/idam/currentEntity/DELETE_FROM_ENTITY_FETCH_SUCCESS';

const fetchErrorDeleteFromEntityAction =
  'mailCheck/admin/idam/currentEntity/DELETE_FROM_FETCH_ERROR';

const fetchRequestAddToEntityAction =
  'mailCheck/admin/idam/currentEntity/ADD_TO_FETCH_REQUEST';

const fetchSuccessAddToEntityAction =
  'mailCheck/admin/idam/currentEntity/ADD_TO_FETCH_SUCCESS';

const fetchErrorAddToEntityAction =
  'mailCheck/admin/idam/currentEntity/ADD_TO_FETCH_ERROR';

export const selectIdamCurrentEntity = createAction(
  selectIdamCurrentEntityAction
);

export const fetchRequestEntity = createAction(fetchRequestEntityAction);

export const fetchSuccessEntity = createAction(fetchSuccessEntityAction);

export const fetchErrorEntity = createAction(fetchErrorEntityAction);

export const fetchRequestDeleteFromEntity = createAction(
  fetchRequestDeleteFromEntityAction
);

export const fetchSuccessDeleteFromEntity = createAction(
  fetchSuccessDeleteFromEntityAction
);

export const fetchErrorDeleteFromEntity = createAction(
  fetchErrorDeleteFromEntityAction
);

export const fetchRequestAddToEntity = createAction(
  fetchRequestAddToEntityAction
);

export const fetchSuccessAddToEntity = createAction(
  fetchSuccessAddToEntityAction
);

export const fetchErrorAddToEntity = createAction(fetchErrorAddToEntityAction);

export const fetchEntities = (current, type) => async dispatch => {
  try {
    dispatch(fetchRequestEntity(type));
    const response = await mailCheckApiFetch(
      `/admin/${current.type}/${current.id}/${type}`
    );
    dispatch(fetchSuccessEntity({ type, entities: response }));
  } catch (error) {
    dispatch(fetchErrorEntity({ type, error }));
  }
};

export const fetchDeleteFromEntity = (current, type, ids) => async dispatch => {
  try {
    dispatch(fetchRequestDeleteFromEntity(type));
    await mailCheckApiFetch(
      `/admin/${current.type}/${current.id}/${type}`,
      'DELETE',
      ids
    );
    dispatch(fetchSuccessDeleteFromEntity({ type, ids }));
  } catch (error) {
    dispatch(fetchErrorDeleteFromEntity({ type, error }));
  }
};

export const fetchAddToEntity = (current, type, ids) => async dispatch => {
  try {
    dispatch(fetchRequestAddToEntity(type));
    await mailCheckApiFetch(
      `/admin/${current.type}/${current.id}/${type}`,
      'PATCH',
      ids
    );
    dispatch(fetchSuccessAddToEntity({ type, ids }));
  } catch (error) {
    dispatch(fetchErrorAddToEntity({ type, error }));
  }
};

const errorReducer = (state, action) => ({
  ...state,
  entities: {
    ...state.entities,
    [action.payload.type]: {
      ...state.entities[action.payload.type],
      error: action.payload.error.message,
      isLoading: false,
    },
  },
});

export const initialState = { id: '', type: '', entities: {} };

export default handleActions(
  {
    [selectIdamCurrentEntityAction]: (state, action) => ({
      ...action.payload,
      entities: {},
    }),

    [fetchRequestEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload]: {
          isLoading: true,
        },
      },
    }),

    [fetchErrorEntityAction]: errorReducer,

    [fetchSuccessEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload.type]: {
          results: map(action.payload.entities, 'id'),
          isLoading: false,
        },
      },
    }),

    [fetchRequestDeleteFromEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload]: {
          ...state.entities[action.payload],
          isLoading: true,
        },
      },
    }),

    [fetchSuccessDeleteFromEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload.type]: {
          results: state.entities[action.payload.type].results.filter(
            id => !includes(action.payload.ids, id)
          ),
          isLoading: false,
        },
      },
    }),

    [fetchErrorDeleteFromEntityAction]: errorReducer,

    [fetchRequestAddToEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload]: {
          ...state.entities[action.payload.type],
          isLoading: true,
        },
      },
    }),

    [fetchSuccessAddToEntityAction]: (state, action) => ({
      ...state,
      entities: {
        ...state.entities,
        [action.payload.type]: {
          results: state.entities[action.payload.type].results.concat(
            action.payload.ids
          ),
          isLoading: false,
        },
      },
    }),

    [fetchErrorAddToEntityAction]: errorReducer,
  },
  initialState
);
export const getIdamCurrentEntity = state => state.admin.idam.currentEntity;
