import { defineStore } from 'pinia'
import { ref } from 'vue'
import { friendsApi } from '@/services/api'
import { useAuthStore } from './auth'

export const useFriendsStore = defineStore('friends', () => {
  const friends = ref<Array<{ id: string; email: string; fullName: string }>>([])
  const pending = ref<Array<{ id: number; requesterId: string; addresseeId: string; status: string }>>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function loadFriends() {
    const auth = useAuthStore()
    if (!auth.user) return
    try {
      isLoading.value = true
      friends.value = await friendsApi.getFriends(auth.user.id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Ошибка загрузки друзей'
    } finally {
      isLoading.value = false
    }
  }

  async function loadPending() {
    const auth = useAuthStore()
    if (!auth.user) return
    try {
      isLoading.value = true
      pending.value = await friendsApi.getPending(auth.user.id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Ошибка загрузки заявок'
    } finally {
      isLoading.value = false
    }
  }

  async function sendRequest(addresseeEmail: string) {
    const auth = useAuthStore()
    if (!auth.user) return
    await friendsApi.sendRequest(auth.user.id, addresseeEmail)
    await loadPending()
  }

  async function accept(friendshipId: number) {
    await friendsApi.accept(friendshipId)
    await Promise.all([loadFriends(), loadPending()])
  }

  async function reject(friendshipId: number) {
    await friendsApi.reject(friendshipId)
    await loadPending()
  }

  return { friends, pending, isLoading, error, loadFriends, loadPending, sendRequest, accept, reject }
})


