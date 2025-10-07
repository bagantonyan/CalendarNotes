<template>
  <div class="friends-page">
    <div class="container">
      <h1>Друзья</h1>

      <div class="add-friend">
        <input v-model="email" type="email" placeholder="Email друга" />
        <button @click="addFriend" :disabled="!email.trim()">Добавить</button>
      </div>

      <div class="lists">
        <div class="friends-list">
          <h3>Мои друзья</h3>
          <div v-if="friendsStore.isLoading">Загрузка...</div>
          <ul>
            <li v-for="f in friendsStore.friends" :key="f.id" class="friend-item">
              <div>
                <div class="name">{{ f.fullName || f.email }}</div>
                <div class="email">{{ f.email }}</div>
              </div>
              <button @click="startChat(f.id)">Написать</button>
            </li>
          </ul>
        </div>

        <div class="pending-list">
          <h3>Заявки в друзья</h3>
          <ul>
            <li v-for="p in friendsStore.pending" :key="p.id" class="pending-item">
              <span>Заявка #{{ p.id }} от {{ p.requesterId }}</span>
              <div class="actions">
                <button @click="accept(p.id)">Принять</button>
                <button @click="reject(p.id)">Отклонить</button>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
  
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useFriendsStore } from '@/stores/friends'
import { useChatStore } from '@/stores/chat'
import { useRouter } from 'vue-router'

const friendsStore = useFriendsStore()
const chatStore = useChatStore()
const router = useRouter()

const email = ref('')

onMounted(async () => {
  await Promise.all([friendsStore.loadFriends(), friendsStore.loadPending()])
})

async function addFriend() {
  await friendsStore.sendRequest(email.value)
  email.value = ''
}

async function accept(id: number) {
  await friendsStore.accept(id)
}

async function reject(id: number) {
  await friendsStore.reject(id)
}

async function startChat(friendId: string) {
  // Получаем/создаем приватную комнату и переходим в /chat
  const room = await chatStore.createPrivateRoom(friendId)
  if (room) {
    await chatStore.selectRoom(room)
    router.push({ name: 'chat' })
  }
}
</script>

<style scoped>
.friends-page { padding: 20px; }
.container { max-width: 900px; margin: 0 auto; }
.add-friend { display: flex; gap: 8px; margin-bottom: 16px; }
.add-friend input { flex: 1; padding: 8px; }
.lists { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.friend-item, .pending-item { display: flex; justify-content: space-between; align-items: center; padding: 8px; border: 1px solid #eee; border-radius: 6px; margin-bottom: 8px; }
.name { font-weight: 600; }
.email { font-size: 12px; color: #666; }
.actions { display: flex; gap: 8px; }
</style>


