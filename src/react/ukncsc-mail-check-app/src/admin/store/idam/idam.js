import { combineReducers } from 'redux';
import currentEntity from './current-entity';
import entities from './entities';
import entitySuggestions from './entity-suggestions';
import searchSuggestions from './search-suggestions';

export default combineReducers({
  currentEntity,
  entities,
  entitySuggestions,
  searchSuggestions,
});
