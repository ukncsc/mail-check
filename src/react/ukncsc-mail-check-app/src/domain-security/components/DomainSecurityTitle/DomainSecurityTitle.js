import React from 'react';
import PropTypes from 'prop-types';
import { Header, Icon, Loader } from 'semantic-ui-react';
import moment from 'moment';
import { getTitleIconProps } from './DomainSecurityTitle.helpers';

import './DomainSecurityTitle.css';

const DomainSecurityTitle = ({
  title,
  loading,
  error,
  subtitle,
  failures,
  warnings,
  inconclusives,
  pending,
  lastChecked,
}) => (
  <React.Fragment>
    <Header as="h1">
      {loading && <Loader active inline style={{ marginRight: 20 }} />}
      {!loading && (
        <Icon
          {...getTitleIconProps(
            error,
            failures,
            warnings,
            inconclusives,
            pending
          )}
        />
      )}
      <Header.Content className="DomainSecurityTitle--header">
        {title}
        {subtitle && <Header.Subheader>{subtitle}</Header.Subheader>}
      </Header.Content>
    </Header>
    {lastChecked && <p>Last checked {moment(lastChecked).fromNow()}</p>}
  </React.Fragment>
);

DomainSecurityTitle.defaultProps = {
  title: '',
  loading: false,
  subtitle: null,
  failures: [],
  warnings: [],
  inconclusives: [],
  pending: false,
};

DomainSecurityTitle.propTypes = {
  title: PropTypes.string,
  loading: PropTypes.bool,
  subtitle: PropTypes.string,
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
  pending: PropTypes.bool,
};

export default DomainSecurityTitle;
