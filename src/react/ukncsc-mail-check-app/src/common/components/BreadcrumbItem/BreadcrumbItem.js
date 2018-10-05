import React from 'react';
import { Breadcrumb } from 'semantic-ui-react';
import { Link } from 'react-router-dom';

const BreadcrumbItem = ({ link, children, ...props }) => (
  <React.Fragment>
    <Breadcrumb.Divider icon="right angle" />
    <Breadcrumb.Section {...props}>
      {link ? <Link to={link}>{children}</Link> : children}
    </Breadcrumb.Section>
  </React.Fragment>
);

export default BreadcrumbItem;
