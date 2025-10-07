import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/Home.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/Login.vue'),
      meta: { requiresAuth: false, guestOnly: true }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/Register.vue'),
      meta: { requiresAuth: false, guestOnly: true }
    },
    {
      path: '/notes',
      name: 'notes',
      component: () => import('@/views/Notes.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/notes/create',
      name: 'create-note',
      component: () => import('@/views/CreateNote.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/notes/:id/edit',
      name: 'edit-note',
      component: () => import('@/views/EditNote.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/NotFound.vue')
    }
  ]
})

// Navigation guards
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    // Требуется аутентификация, но пользователь не авторизован
    next({ name: 'login', query: { redirect: to.fullPath } })
  } else if (to.meta.guestOnly && authStore.isAuthenticated) {
    // Страница только для гостей, но пользователь авторизован
    next({ name: 'notes' })
  } else {
    next()
  }
})

export default router

