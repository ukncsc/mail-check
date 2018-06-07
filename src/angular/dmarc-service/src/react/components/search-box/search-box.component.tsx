import * as React from 'react';

export default ({title, placeHolder, handleSearchChanged}) => (
  <form className="filter-form">
    <div className="form-group">
      <label className="col-md-6 col-form-label col-form-label-lg">{title}</label>
      <div className="input-group input-group-lg test">
        <input type="text" onChange={handleSearchChanged} className="form-control form-control-lg" id="inlineFormInputGroup" placeholder={placeHolder}/>
        <div className="input-group-addon">
          <i className="fa fa-search" aria-hidden="true"></i>
        </div>
      </div>
    </div>
  </form>
);