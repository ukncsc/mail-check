import * as React from 'react';
import User from '../user';

export default ({ companyLogo, productName, user }) => (
    <div style={{background: "white"}}>
        <nav className="container header">
            <div className="row">
                <div className="col-md-6">
                    <a className="navbar-brand" href="/app/">
                        <img src={companyLogo} alt="NCSC" />
                    </a>
                </div>
                <div className="col-md-6">
                    <div className="float-right">
                        <User {...user} />
                    </div>
                    <div className="float-right row-align-bottom">
                        <h3>{productName}</h3>
                    </div>
                </div>
            </div>
        </nav>
    </div>
);