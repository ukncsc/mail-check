import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Accordion, Icon } from 'semantic-ui-react';

import './ShowMoreDropdown.css';

class ShowMoreDropdown extends Component {
  state = { showMore: false };

  toggleContent = () => this.setState({ showMore: !this.state.showMore });

  render() {
    const { children, title } = this.props;
    const { showMore } = this.state;

    return (
      <Accordion>
        <Accordion.Title
          active={showMore}
          onClick={this.toggleContent}
          className="ShowMoreDropdown--title"
        >
          <Icon name="dropdown" className="ShowMoreDropdown--icon" />
          <span className="ShowMoreDropdown--title-text">{title}</span>
        </Accordion.Title>
        <Accordion.Content active={showMore}>
          <div className="ShowMoreDropdown--content">{children}</div>
        </Accordion.Content>
      </Accordion>
    );
  }
}

ShowMoreDropdown.defaultProps = {
  title: 'Show more',
};

ShowMoreDropdown.propTypes = {
  title: PropTypes.string,
};

export default ShowMoreDropdown;
