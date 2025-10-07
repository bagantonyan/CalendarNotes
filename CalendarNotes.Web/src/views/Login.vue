<template>
  <div style="max-width: 400px; margin: 60px auto">
    <n-card title="Вход в систему">
      <n-form ref="formRef" :model="formValue" :rules="rules" @submit.prevent="handleLogin">
        <n-form-item label="Email" path="email">
          <n-input v-model:value="formValue.email" placeholder="example@email.com" />
        </n-form-item>
        <n-form-item label="Пароль" path="password">
          <n-input
            v-model:value="formValue.password"
            type="password"
            show-password-on="click"
            placeholder="Введите пароль"
          />
        </n-form-item>
        <n-space vertical :size="16">
          <n-button
            type="primary"
            block
            :loading="authStore.loading"
            attr-type="submit"
            @click="handleLogin"
          >
            Войти
          </n-button>
          <n-button block @click="router.push('/register')">
            Зарегистрироваться
          </n-button>
        </n-space>
      </n-form>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { NCard, NForm, NFormItem, NInput, NButton, NSpace, useMessage } from 'naive-ui'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const message = useMessage()

const formValue = ref({ email: '', password: '' })
const rules = {
  email: { required: true, message: 'Введите email', trigger: 'blur' },
  password: { required: true, message: 'Введите пароль', trigger: 'blur' }
}

async function handleLogin() {
  try {
    await authStore.login(formValue.value)
    message.success('Успешный вход!')
    router.push('/notes')
  } catch (error: any) {
    message.error(error.response?.data?.message || 'Ошибка входа')
  }
}
</script>

