import * as React from 'react';
//import Range from 'lodash';


export default class Pagination extends React.Component<any, any> {
  constructor(public props) {
    super(props)
  }

  render() {
    let collectionSize = this.props.collectionSize;
    let pageSize = this.props.pageSize;
    let page = this.props.page;

    let maxSize = 5;

    let pageCount = Math.ceil(collectionSize / pageSize);
    let pages: number[] = [];

    if (!pageCount) {
      pageCount = 0;
    }

    for (let i = 1; i <= pageCount; i++) {
      pages.push(i);
    }

    if (maxSize > 0 && pageCount > maxSize) {
      let start = 0;
      let end = pageCount;
      let leftOffset = Math.floor(maxSize / 2);
      let rightOffset = maxSize % 2 === 0 ? leftOffset - 1 : leftOffset;
  
      if (page <= leftOffset) {
        end = maxSize;
      } else if (pageCount - page < leftOffset) {
        start = pageCount - maxSize;
      } else {
        start = page - leftOffset - 1;
        end = page + rightOffset;
      }
  
      pages = pages.slice(start, end);

      if (start > 0) {
        if (start > 1) {
          pages.unshift(-1);
        }
        pages.unshift(1);
      }
      if (end < pageCount) {
        if (end < (pageCount - 1)) {
          pages.push(-1);
        }
        pages.push(pageCount);
      }
    }

    return (
      <div className="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
        <div className="btn-group mr-2" role="group" aria-label="First group">
          {pages.map((v, i) => {
            if(v === -1){
              return <button type="button" className="btn btn-secondary">...</button>
            }
            else{
              return <button type="button" className={"btn " + (v === page ? "btn-primary" : "btn-secondary")} onClick={() => this.props.selectPage(v)}>{v}</button>
            }
          })}
        </div>
      </div>      
    )
  }
}