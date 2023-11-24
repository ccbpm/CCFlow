import { Fragment } from "react";
import { userStore } from "../store/userStore";
import { Navigate } from "react-router-dom";

export function RequireAuth({ children }: { children: JSX.Element }) {
    const token = userStore.token;
    return (
        <Fragment>
            {!token ? <Navigate to="/login" replace={true} /> : children}
        </Fragment>
    )
}


export function LoginPageGuard({ children }: { children: JSX.Element }) {
    const token = userStore.token;
    return (
        <Fragment>
            {token ? <Navigate to="/" replace={true} /> : children}
        </Fragment>
    )
}