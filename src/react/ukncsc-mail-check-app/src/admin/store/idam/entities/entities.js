import { createAction, handleActions } from 'redux-actions';
import get from 'lodash/get';
import keyBy from 'lodash/keyBy';
import map from 'lodash/map';
import mapValues from 'lodash/mapValues';
import omit from 'lodash/omit';
import { fetchSuccessEntityAction } from 'admin/store/idam/current-entity';
import { fetchSuccessIdamSearchSuggestionsAction } from 'admin/store/idam/search-suggestions';

const addIdamEntitiesAction = 'mailCheck/admin/idam/entities/ADD';

const updateIdamEntityAction = 'mailCheck/admin/idam/entities/UPDATE';

const removeIdamEntitiesAction = 'mailCheck/admin/idam/entities/REMOVE';

export const addIdamEntities = createAction(addIdamEntitiesAction);

export const removeIdamEntities = createAction(removeIdamEntitiesAction);
const addIdamEntitiesReducer = (state, action) => ({
  ...state,
  [action.payload.type]: {
    ...state[action.payload.type],
    ...keyBy(action.payload.entities, 'id'),
  },
});

const initialState = {};

export default handleActions(
  {
    [addIdamEntitiesAction]: addIdamEntitiesReducer,

    [updateIdamEntityAction]: (state, action) => ({
      ...state,
      [action.payload.type]: {
        ...state[action.payload.type],
        [action.payload.id]: {
          ...state[action.payload.type][action.payload.id],
          ...action.payload.entity,
        },
      },
    }),

    [removeIdamEntitiesAction]: (state, action) =>
      omit(
        state,
        map(action.payload.ids, id => `${action.payload.type}.${id}`)
      ),

    [fetchSuccessEntityAction]: addIdamEntitiesReducer,

    [fetchSuccessIdamSearchSuggestionsAction]: (state, action) => ({
      ...state,
      ...mapValues(action.payload, (entities, type) => ({
        ...state[type],
        ...keyBy(entities.results, 'id'),
      })),
    }),
  },
  initialState
);
export const getIdamUsers = state => state.admin.idam.entities.user;

export const getIdamGroups = state => state.admin.idam.entities.group;

export const getIdamDomains = state => state.admin.idam.entities.domain;

export const getIdamUserById = state => id =>
  get(state, `admin.idam.entities.user[${id}]`, {});

export const getIdamGroupById = state => id =>
  get(state, `admin.idam.entities.group[${id}]`, {});

export const getIdamDomainById = state => id =>
  get(state, `admin.idam.entities.domain[${id}]`, {});
