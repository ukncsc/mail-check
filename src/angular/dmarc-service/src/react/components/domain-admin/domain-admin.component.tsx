import * as React from 'react';
import { connect } from 'react-redux';

import { default as DomainList } from '../../components/domain-list';
import { default as Pagination } from '../../components/pagination';
import { default as PaginationDisplay } from '../../components/pagination-display';
import { default as SearchBox } from '../../components/search-box';


export default class DomainAdmin extends React.Component<any, any> {

  constructor(public props) {
    super(props);
  }

  render() {
    return (
      <div>
        <h3>Domain</h3>
        <div className="row">
          <div className="col-md-12">
            <SearchBox
              title="Search from a database of all Mail Check domains"
              placeHolder="Search domains"
              handleSearchChanged={this.props.handleSearchChanged} />
          </div>
        </div>
        <div className="row">
          <div className="col-md-3">
            <PaginationDisplay
              entityType="Domains"
              {...this.props} />
          </div>
          <div className="col-md-9">
            <DomainList
              {...this.props} />
          </div>
        </div>
        <div className="row">
          <div className="offset-md-6 col-md-6">
            <span className="float-right">
              <Pagination
                selectPage={this.props.selectPage}
                {...this.props} />
            </span>
          </div>
        </div>
      </div>
    )
  }
}
