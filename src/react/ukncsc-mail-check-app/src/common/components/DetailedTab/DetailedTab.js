import React, { Component } from 'react';
import { Menu } from 'semantic-ui-react';

import './DetailedTab.css';

class DetailedTab extends Component {
  componentWillMount() {
    const { children, index, tabSelected } = this.props;
    if (index === 0) {
      tabSelected(children, index);
    }
  }

  render() {
    const {
      title,
      headline,
      subtitle,
      index,
      activeIndex,
      tabSelected,
      children,
    } = this.props;
    return (
      <Menu.Item
        active={index === activeIndex}
        onClick={() => tabSelected(children, index)}
        className="DetailedTab"
      >
        <div className="DetailedTab--content">
          <p>{title}</p>
          <h1>{headline}</h1>
          <h5>{subtitle}</h5>
        </div>
      </Menu.Item>
    );
  }
}

export default DetailedTab;
