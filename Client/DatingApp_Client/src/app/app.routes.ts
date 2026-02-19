import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MembersList } from '../features/members/members-list/members-list';
import { MembersDetailed } from '../features/members/members-detailed/members-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },

    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [{
            path: 'member/:id',
            component: MembersDetailed
        },
        {
            path: 'lists',
            component: Lists
        },
        {
            path: 'messages',
            component: Messages
        },
        {
            path: 'members',
            component: MembersList,
        },
        ]
    },

    {
        path:'errors',
        component:TestErrors
    },

    {
        path:'server-error',
        component:ServerError
    },

    // root that doesnt exist
    {
        path: '**',
        component: NotFound
    }
];
