import * as React from 'react';

export default ({ info }) => 
  info ? 
    (
      <div>
        <h4 className="inline">
          <i className="fa fa-user-circle" aria-hidden="true"/>
        </h4>
        <h6 style={{ verticalAlign: "3px" }} className="inline">
          {info.name} | <a href="/callback?logout=">Logout</a>
        </h6>
      </div>
    )
    : null;