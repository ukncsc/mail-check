import React, { Component } from 'react';
import { Menu, Segment } from 'semantic-ui-react';

import './DetailedTabList.css';

class DetailedTabList extends Component {
  state = {
    selectedTab: null,
    activeIndex: 0,
  };

  tabSelected = (selectedTab, activeIndex) =>
    this.setState({ selectedTab, activeIndex });

  render() {
    const { children } = this.props;
    const { selectedTab, activeIndex } = this.state;

    return (
      <React.Fragment>
        <Menu
          tabular
          stackable
          widths={React.Children.count(children) || 1}
          className="DetailedTabList"
          attached="top"
        >
          {React.Children.map(children, (child, index) =>
            React.cloneElement(child, {
              index,
              activeIndex,
              tabSelected: this.tabSelected,
            })
          )}
        </Menu>
        <Segment attached="bottom">{selectedTab}</Segment>
      </React.Fragment>
    );
  }
}

export default DetailedTabList;
