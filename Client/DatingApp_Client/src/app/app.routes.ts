import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MembersList } from '../features/members/members-list/members-list';
import { MembersDetailed } from '../features/members/members-detailed/members-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';

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



    // root that doesnt exist
    {
        path: '**',
        component: Home
    }
];
